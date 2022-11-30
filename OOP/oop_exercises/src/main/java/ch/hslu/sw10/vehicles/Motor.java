package ch.hslu.sw10.vehicles;

import ch.hslu.sw04.Switchable;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.util.ArrayList;
import java.util.List;

/**
 * Simple class for simulating a motor
 */
public class Motor implements Switchable{
    private boolean isOn = false;
    private final int startRpm = 1600;
    private int rpm = 0;

    private static Logger logger = LogManager.getLogger(Motor.class);
    private final List<PropertyChangeListener> changeListeners = new ArrayList<>();

    /**
     * Returns current rotations per minute
     * @return rpm
     */
    public int getRpm() {
        return this.rpm;
    }

    /**
     * Increase or decrease motor speed
     * @param acceleration  acceleration in rpm/s
     * @param time          time in seconds
     */
    public void accelerate(int acceleration, int time) {

        this.rpm += acceleration * time;
    }

    /**
     * Add a property change listener.
     * @param listener
     */
    public void addPropertyChangeListener(final PropertyChangeListener listener) {
        if(listener != null) {
            this.changeListeners.add(listener);
        }
    }

    /**
     * Remove a property change listener.
     * @param listener
     */
    public void removePropertyChangeListener(final PropertyChangeListener listener) {
        this.changeListeners.remove(listener);
    }

    /**
     * Switch motor on.
     */
    @Override
    public void switchOn(){
        this.isOn = true;
        this.rpm = this.startRpm;

        PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "off", "on");
        this.firePropertyChangeEvent(pcEvent);
    }

    /**
     * Switch motor off.
     */
    @Override
    public void switchOff(){
        if(this.isSwitchedOn()) {
            this.isOn = false;
            this.rpm = 0;

            PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "on", "off");
            this.firePropertyChangeEvent(pcEvent);
        }
    }

    @Override
    public boolean isSwitchedOn(){
        return this.isOn;
    }

    @Override
    public boolean isSwitchedOff(){
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

    private void firePropertyChangeEvent(final PropertyChangeEvent pcEvent){
        for(PropertyChangeListener listener : this.changeListeners){
            listener.propertyChange(pcEvent);
        }
    }
}
