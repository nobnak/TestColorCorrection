using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ColorCurve {
    public enum IndexEnum { Black = 0, Shadow, Highlight, White }

    public static readonly float[] BASE_VALUES;

    protected readonly AnimationCurve curve;
    protected readonly AnimationUtility.TangentMode tangentMode;
    protected readonly Keyframe[] keys;
    protected readonly float[] offsets;

    static ColorCurve() {
        var values = System.Enum.GetValues (typeof(IndexEnum));
        BASE_VALUES = new float[values.Length];
        for (var i = 0; i < BASE_VALUES.Length; i++)
            BASE_VALUES [i] = (float)i / (BASE_VALUES.Length - 1);
    }

    public ColorCurve(AnimationUtility.TangentMode tangentMode) {
        this.tangentMode = tangentMode;

        this.keys = GenerateKeys ();

        var curve = new AnimationCurve (keys);
        curve.preWrapMode = WrapMode.Clamp;
        curve.postWrapMode = WrapMode.Clamp;
        this.curve = curve;

        this.offsets = new float[keys.Length];

        ApplyKeys ();
        ApplyTangentMode ();
    }
    public ColorCurve() : this(AnimationUtility.TangentMode.Linear) {}

    public void SetOffset(IndexEnum index, float value) {
        var i = (int)index;
        offsets [i] = value;
        ApplyKeys ();
    }
    public float GetOffset(IndexEnum index) {
        var i = (int)index;
        return offsets [i];
    }

    public float Evaluate(float t) {
        return curve.Evaluate (t);
    }

    public void ApplyOffset () {
        for (var i = 0; i < keys.Length; i++)
            keys [i].value = offsets [i] + BASE_VALUES [i];
    }
    public void ApplyKeys() {
        ApplyOffset ();
        curve.keys = keys;
    }
    public void ApplyTangentMode() {
        for (var i = 0; i < keys.Length; i++)
            AnimationUtility.SetKeyLeftTangentMode (curve, i, tangentMode);
    }

    public void DrawGUI() {
        GUILayout.BeginVertical ();

        var key = IndexEnum.White;
        var val = GetOffset (key);
        GUILayout.Label (System.Enum.GetName (typeof(IndexEnum), key));
        val = GUILayout.HorizontalSlider (val, -1f, 1f);

        GUILayout.EndVertical ();
    }

    protected static Keyframe[] GenerateKeys () {
        var keys = new Keyframe[BASE_VALUES.Length];
        for (var i = 0; i < BASE_VALUES.Length; i++) {
            var v = BASE_VALUES [i];
            var key = new Keyframe (v, v);
            keys [i] = key;
        }
        return keys;
    }
}
