package ch.hslu.sw10;

import ch.hslu.sw04.Switchable;

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

    public void addPropertyChangeListener(final PropertyChangeListener listener) {
        if(listener != null) {
            this.changeListeners.add(listener);
        }
    }

    public void removePropertyChangeListener(final PropertyChangeListener listener) {
        this.changeListeners.remove(listener);
    }

    @Override
    public void switchOn(){
        this.isOn = true;
        this.rpm = this.startRpm;

        PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "off", "on");
        this.firePropertyChangeEvent(pcEvent);
    }

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
