package ch.hslu.sw10.vehicles;

import ch.hslu.sw04.Switchable;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;

public final class Vehicle implements Switchable, PropertyChangeListener {
    private Motor motor;
    private Lights lightFrontRight;
    private Lights lightFrontLeft;
    private boolean isOn = false;

    private static Logger logger = LogManager.getLogger(Vehicle.class);

    public Vehicle(){
        this.motor = new Motor();
        this.lightFrontRight = new Lights();
        this.lightFrontLeft = new Lights();

        this.motor.addPropertyChangeListener(this);
        this.lightFrontRight.addPropertyChangeListener(this);
        this.lightFrontLeft.addPropertyChangeListener(this);
    }

    /**
     * Switches on the car and its components.
     */
    @Override
    public void switchOn() {
        if(this.isSwitchedOff()) {
            this.isOn = true;
            this.motor.switchOn();
            this.lightFrontRight.switchOn();
            this.lightFrontLeft.switchOn();
        } else {
            this.logger.info("Already switched on");
        }
    }

    /**
     * Switches off the car and its components.
     */
    @Override
    public void switchOff() {
        if(this.isSwitchedOn()) {
            this.isOn = false;
            this.motor.switchOff();
            this.lightFrontRight.switchOff();
            this.lightFrontLeft.switchOff();
        } else {
            this.logger.info("Already switched off");
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

    @Override
    public void propertyChange(PropertyChangeEvent evt) {
        if(evt.getSource() == this.motor){
            this.handleMotorEvent(evt);
        } else if (evt.getSource() == this.lightFrontLeft) {
            this.handleLightEvent(evt, LightsPosition.FRONT_LEFT);
        } else if (evt.getSource() == this.lightFrontRight){
            this.handleLightEvent(evt, LightsPosition.FRONT_RIGHT);
        }
    }

    private void handleMotorEvent(PropertyChangeEvent evt){
        String output = "Motor state: " + evt.getOldValue() + " -> " + evt.getNewValue();
        this.logger.info(output);
    }

    private void handleLightEvent(PropertyChangeEvent evt, LightsPosition place){
        String output = place.getPos() + " light: " + evt.getOldValue() + " -> " + evt.getNewValue();
        this.logger.info(output);
    }
}
