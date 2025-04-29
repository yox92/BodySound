namespace BodySound.Utils;

/// <summary>
/// AudioClip information for volume adjustment.
/// </summary>
public readonly struct ClipVolumeInfo(string name, float volume, bool skipWeightAdjust)
{
    public readonly string Name = name;
    public readonly float Volume = volume;
    public readonly bool SkipWeightAdjust = skipWeightAdjust;
}