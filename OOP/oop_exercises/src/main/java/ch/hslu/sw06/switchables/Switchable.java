package ch.hslu.sw06.switchables;
/**
 * Simple interface for any switchable object.
 */
public interface Switchable {
    /**
     * Turn the object on.
     */
    void switchOn();

    /**
     * Turn the object off.
     */
    void switchOff();

    /**
     * Check if object is switched on.
     * @return boolean if object is switched off.
     */
    boolean isSwitchedOn();

    /**
     * Check if object is switched off.
     * @return boolean if object is switched off.
     */
    boolean isSwitchedOff();
}
