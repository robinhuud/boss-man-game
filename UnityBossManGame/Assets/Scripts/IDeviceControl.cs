//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

public interface IDeviceControl
{
    // method should toggle whatever internal state, and activate any animations
    // or state changes that happen as a result
    void Activate();
    // return whether this object is "on" (activated) or "off" (not activated)
    bool IsActive();
}
