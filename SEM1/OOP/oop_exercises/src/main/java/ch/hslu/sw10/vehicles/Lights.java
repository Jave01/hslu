package ch.hslu.sw10.vehicles;

import ch.hslu.sw04.Switchable;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.util.ArrayList;
import java.util.List;

public class Lights implements Switchable {
    private boolean isOn = false;
    private int cycles;
    private final List<PropertyChangeListener> changeListeners = new ArrayList<>();

    @Override
    public void switchOn() {
        if(this.isSwitchedOff()) {
            this.isOn = true;

            PropertyChangeEvent pcEvent = new PropertyChangeEvent(this, "status", "off", "on");
            this.firePropertyChangeEvent(pcEvent);
        }
    }

    @Override
    public void switchOff() {
        if (this.isSwitchedOn()){
            this.isOn = false;
            this.cycles += 1;

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
        return !this.isSwitchedOn();
    }

    public void addPropertyChangeListener(final PropertyChangeListener listener) {
        this.changeListeners.add(listener);
    }

    public void removePropertyChangeListener(final PropertyChangeListener listener) {
        this.changeListeners.remove(listener);
    }

    public void firePropertyChangeEvent(final PropertyChangeEvent pcEvent){
        for(PropertyChangeListener listener : this.changeListeners){
            listener.propertyChange(pcEvent);
        }
    }
}
