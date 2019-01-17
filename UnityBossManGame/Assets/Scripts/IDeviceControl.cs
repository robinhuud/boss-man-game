//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

/// <summary>
/// Generic interface for gameobjects that have an "activation" state.
/// Activation can be set and queried using the public Activate() and IsActive() methods respectively.
/// 
/// These methods are called from the gaze_control script when it encounters an object tagged TOUCHABLE
/// </summary>

public interface IDeviceControl
{
    // method should toggle whatever internal state, and activate any animations
    // or state changes that happen as a result
    void Activate();
    // return whether this object is "on" (activated) or "off" (not activated)
    bool IsActive();
}
