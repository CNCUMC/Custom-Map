param(
    [string]$GamePath, # 游戏路径
    [string]$ModNamespace, # 模组命名空间
    [string]$ModName
)

# 设置编码为 UTF-8
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::InputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# 强制设置当前代码页为 UTF-8 (65001)
chcp 65001 > $null

# 获取时间戳
$timestamp = Get-Date -Format "yyyy-MM-dd_HH.mm.ss"

# 各种路径
$GamePath = [System.IO.Path]::GetFullPath($GamePath) # 游戏路径
$bepInExPath = [System.IO.Path]::Combine($GamePath, "BepInEx")

# 各种文件
$bepInExLog = [System.IO.Path]::Combine($bepInExPath, "LogOutput.log") # BepInEx 日志
$GameExecutable = [System.IO.Path]::Combine($GamePath, "CasualtiesUnknown.exe") # 游戏文件
$ModDll = [System.IO.Path]::Combine($PSScriptRoot, "bin/Debug/net48", "$ModNamespace.dll")

# 统一使用 ModName 作为目标文件夹名称
$targetModFolder = $ModName

# Lang 文件夹路径
$sourceLangPath = [System.IO.Path]::Combine($PSScriptRoot, "Lang") # 源 Lang 文件夹
$destLangPath = [System.IO.Path]::Combine($bepInExPath, "plugins", $targetModFolder, "Lang") # 目标 Lang 文件夹

# 文档文件列表
$docFiles = @("README.md", "README_ZH.md", "LICENSE.md", "Cover.png")

# 日志目标路径
$logDestination = [System.IO.Path]::Combine($PSScriptRoot, "Logs", "$timestamp.log") # 日志目标路径

# 检查游戏路径是否有效
if (-not (Test-Path $GamePath -PathType Container)) {
    Write-Error "Game path invalid or not a directory: $GamePath"
    exit 1
}

# 确保目标目录存在
$logsFolder = [System.IO.Path]::Combine($PSScriptRoot, "Logs")
if (-not (Test-Path $logsFolder)) {
    New-Item -ItemType Directory -Path $logsFolder -Force
}

# 封装输出函数
function Write-ColoredMessage {
    param (
        [string]$Message,
        [System.ConsoleColor]$Color
    )
    Write-Host $Message -ForegroundColor $Color
}

# 定义日志复制函数
function Copy-BepInExLog {
    if (Test-Path $bepInExLog) {
        try {
            Copy-Item $bepInExLog $logDestination -Force
            Write-ColoredMessage "Copying BepInEx logs to ""$logDestination""." Cyan
        }
        catch {
            Write-Warning "Failed to copy BepInEx logs: $_"
        }
    }
}

# 间隔输出
function Interval {
    Write-Host "----------------------------------------"
}

# 清空 BepInEx 日志文件
if (Test-Path $bepInExLog) {
    Clear-Content $bepInExLog
    Write-ColoredMessage "Cleared previous BepInEx logs." Cyan
}

# 输出启动信息
Write-ColoredMessage "Game path: $GamePath" Yellow
Write-ColoredMessage "Mod namespace: $ModNamespace" Yellow
Write-ColoredMessage "Mod name: $ModName" Yellow
Write-ColoredMessage "Target folder: $targetModFolder" Yellow

# 复制dll文件到游戏目录 - 统一使用 ModName 文件夹
try {
    $pluginPath = [System.IO.Path]::Combine($bepInExPath, "plugins", $targetModFolder)
    New-Item -ItemType Directory -Path $pluginPath -Force
    Copy-Item $ModDll ([System.IO.Path]::Combine($pluginPath, "$ModNamespace.dll")) -Force
    Write-ColoredMessage "Copying Mod dll file to ""$pluginPath\$ModNamespace.dll""." Cyan
}
catch {
    Write-Error "Failed to copy Mod dll file: $_"
    exit 1
}

# 处理 Lang 文件夹 - 统一使用 ModName 文件夹
try {
    # 创建目标 Lang 目录
    if (-not (Test-Path $destLangPath)) {
        New-Item -ItemType Directory -Path $destLangPath -Force
        Write-ColoredMessage "Created Lang directory at ""$destLangPath""" Cyan
    }

    # 如果源 Lang 目录存在，则复制所有文件
    if (Test-Path $sourceLangPath -PathType Container) {
        # 先清空目标目录中的现有文件
        Get-ChildItem -Path $destLangPath -File | Remove-Item -Force
        # 复制所有文件（包括子目录）
        Copy-Item -Path "$sourceLangPath\*" -Destination $destLangPath -Recurse -Force
        Write-ColoredMessage "Successfully copied all Lang files from ""$sourceLangPath"" to ""$destLangPath""" Green
    } else {
        Write-ColoredMessage "Source Lang directory not found at ""$sourceLangPath"". Created empty directory." Yellow
    }
}
catch {
    Write-Warning "Failed to process Lang directory: $_"
    exit 1
}

# 验证 Lang 文件是否成功复制
try {
    $copiedFiles = Get-ChildItem -Path $destLangPath -File
    if ($copiedFiles.Count -gt 0) {
        Write-ColoredMessage "Verified copied Lang files:" Cyan
        foreach ($file in $copiedFiles) {
            Write-ColoredMessage "  - $($file.Name)" Cyan
        }
    } else {
        Write-ColoredMessage "Warning: No Lang files were copied!" Yellow
    }
}
catch {
    Write-Warning "Failed to verify copied Lang files: $_"
}

# 复制文档文件到插件目录
try {
    $destDocPath = [System.IO.Path]::Combine($bepInExPath, "plugins", $targetModFolder)
    $copiedDocs = 0
    
    foreach ($docFile in $docFiles) {
        $sourceDocPath = [System.IO.Path]::Combine($PSScriptRoot, $docFile)
        $destDocFilePath = [System.IO.Path]::Combine($destDocPath, $docFile)
        
        if (Test-Path $sourceDocPath -PathType Leaf) {
            Copy-Item $sourceDocPath $destDocFilePath -Force
            Write-ColoredMessage "Copying document file ""$docFile"" to ""$destDocFilePath""." Cyan
            $copiedDocs++
        } else {
            Write-ColoredMessage "Document file ""$docFile"" not found, skipping." Yellow
        }
    }
    
    if ($copiedDocs -gt 0) {
        Write-ColoredMessage "Successfully copied $copiedDocs document file(s) to plugin directory." Green
    }
}
catch {
    Write-Warning "Failed to copy document files: $_"
}

# 启动游戏进程并重定向输出
try {
    $gameProcess = Start-Process -FilePath $GameExecutable `
        -WorkingDirectory (Split-Path $GameExecutable -Parent) `
        -PassThru -NoNewWindow

    Write-ColoredMessage "Game process started, PID: $($gameProcess.Id)" Yellow
    Interval

    # 定期轮询日志
    $lastReadPosition = 0
    while (!$gameProcess.HasExited) {
        if (Test-Path $bepInExLog) {
            $content = Get-Content $bepInExLog -ReadCount 0 -Encoding UTF8
            for ($i = $lastReadPosition; $i -lt $content.Count; $i++) {
                Write-ColoredMessage $content[$i] Magenta
            }
            $lastReadPosition = $content.Count
        }
        Start-Sleep -Milliseconds 500 # 每 500ms 检查一次
    }

    # 等待游戏进程退出
    Interval
    Write-ColoredMessage "Game process exited." Red
}

catch {
    Write-Error "Failed to start the game process: $_"
    exit 1
}

finally {
    # 如果游戏进程仍在运行，则终止它
    if ($gameProcess -and !$gameProcess.HasExited) {
        Interval
        Write-ColoredMessage "Terminating game process..." Red
        $gameProcess.Kill()
    }
    Copy-BepInExLog
}
