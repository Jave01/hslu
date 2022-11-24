package ch.hslu.sw10;

import ch.hslu.sw04.Lights;
import ch.hslu.sw04.Switchable;

public class Vehicle implements Switchable {
    Motor motor;
    Lights lights;
    private boolean isOn = false;
    @Override
    public void switchOn() {
        this.isOn = true;
    }

    @Override
    public void switchOff() {
        this.isOn = false;
    }

    @Override
    public boolean isSwitchedOn() {
        return this.isOn;
    }

    @Override
    public boolean isSwitchedOff() {
        return !this.isSwitchedOn();
    }

    public Vehicle(){
        this.motor = new Motor();
        this.lights = new Lights();
    }
}
