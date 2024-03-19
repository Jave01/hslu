package ch.hslu.sw10.vehicles;

import ch.hslu.sw04.Switchable;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

/**
 * Simple class for simulating a motor
 */
public final class Motor implements Switchable {
    private boolean isOn = false;
    private final int startRpm = 1600;
    private int rpm = 0;

    private final List<PropertyChangeListener> changeListeners = new ArrayList<>();

    /**
     * Returns current rotations per minute
     * 
     * @return rpm
     */
    public int getRpm() {
        return this.rpm;
    }

    /**
     * Increase or decrease motor speed
     * 
     * @param acceleration acceleration in rpm
     */
    public void accelerate(int acceleration) {
        if (((float) this.rpm + acceleration) < 0 || this.isSwitchedOff()) {
            this.rpm = 0;
        } else if ((float) this.rpm + acceleration > Integer.MAX_VALUE) {
            this.rpm = Integer.MAX_VALUE;
        } else {
            this.rpm += acceleration;
        }
    }

    /**
     * Add a property change listener.
     * 
     * @param listener
     */
    public void addPropertyChangeListener(final PropertyChangeListener listener) {
        if (listener != null) {
            this.changeListeners.add(listener);
        }
    }

    /**
     * Remove a property change listener.
     * 
     * @param listener
     */
    public void removePropertyChangeListener(final PropertyChangeListener listener) {
        this.changeListeners.remove(listener);
    }

    /**
     * Switch motor on.
     */
    @Override
    public void switchOn() {
        this.isOn = true;
        this.rpm = this.startRpm;

        PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "off", "on");
        this.firePropertyChangeEvent(pcEvent);
    }

    /**
     * Switch motor off and trigger a {@link PropertyChangeEvent}.
     */
    @Override
    public void switchOff() {
        if (this.isSwitchedOn()) {
            this.isOn = false;
            this.rpm = 0;

            PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "on", "off");
            this.firePropertyChangeEvent(pcEvent);
        }
    }

    @Override
    public boolean isSwitchedOn() {
        return this.isOn;
    }

    @Override
    public boolean isSwitchedOff() {
        return !this.isOn;
    }

    @Override
    public String toString() {
        return "Motor{" +
                "isOn=" + isOn +
                ", rpm=" + rpm +
                ", changeListeners=" + changeListeners +
                '}';
    }

    @Override
    public boolean equals(Object o) {
        if (this == o)
            return true;
        if (!(o instanceof Motor other))
            return false;
        return startRpm == other.startRpm;
    }

    @Override
    public int hashCode() {
        return Objects.hash(startRpm);
    }

    /**
     * Execute the propertyChange method on every registered listener.
     * 
     * @param pcEvent the event which will be transmitted.
     */
    private void firePropertyChangeEvent(final PropertyChangeEvent pcEvent) {
        for (PropertyChangeListener listener : this.changeListeners) {
            listener.propertyChange(pcEvent);
        }
    }

}
