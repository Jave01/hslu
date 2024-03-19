package ch.hslu.sw04;

public class Lights implements Switchable{
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
}
