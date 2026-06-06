using System.Reflection;
using CustomFungamePack.Data.Feature.World;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CustomFungamePack.Patch;

[HarmonyPatch(typeof(JumpPadScript))]
public class JumpPadScriptPatch
{
    private static JumpPadData JumpPadData => FungameCheck.CurrentFungame?.JumpPadData;

    private static readonly FieldInfo CooldownField = typeof(JumpPadScript).GetField(
        "cooldown",
        BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo LightField = typeof(JumpPadScript).GetField(
        "light",
        BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPatch("OnCollisionEnter2D")]
    [HarmonyPrefix]
    public static bool OnCollisionPrefix(JumpPadScript __instance, Collision2D collision)
    {
        var data = JumpPadData;
        if (data == null)
            return true;

        if (__instance.disabled || (float)CooldownField.GetValue(__instance) > 0 ||
            !collision.rigidbody || collision.rigidbody.isKinematic)
            return true;

        float force = data.Force;
        var light = (Light2D)LightField.GetValue(__instance);

        if (collision.gameObject.GetComponent<Body>())
        {
            collision.gameObject.GetComponent<Body>().Ragdoll();
        }
        else
        {
            if (collision.gameObject.TryGetComponent(out Limb limbComponent))
            {
                if (light) light.intensity = 1f;
                foreach (Limb limb in limbComponent.body.limbs)
                    limb.rb.velocity = new Vector2(limb.rb.velocity.x, 65f * force);
                limbComponent.body.Scream();
                Sound.Play("jumppad", __instance.transform.position);
                CooldownField.SetValue(__instance, 15f);
                PlayerCamera.main.shaker.Shake(70f);
            }
            else
            {
                if (light) light.intensity = 1f;
                collision.rigidbody.AddForce(Vector2.up * 350f * force, ForceMode2D.Impulse);
                Sound.Play("jumppad", __instance.transform.position);
                CooldownField.SetValue(__instance, 15f);
            }
        }

        CooldownField.SetValue(__instance, data.Cooldown);
        if (data.NoLight && light)
            light.intensity = 0f;

        return false;
    }
}