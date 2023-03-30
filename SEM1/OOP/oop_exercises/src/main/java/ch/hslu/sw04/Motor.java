package ch.hslu.sw04;
/**
 * Simple class for simulating a motor
 */
public class Motor implements Switchable {
    private boolean isOn = false;
    private final int startRpm = 1600;
    private int rpm = 0;

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

    @Override
    public void switchOn(){
        this.isOn = true;
        this.rpm = this.startRpm;
        System.out.println("Turned on");
    }

    @Override
    public void switchOff(){
        this.isOn = false;
        this.rpm = 0;
        System.out.println("Turned off");
    }

    @Override
    public boolean isSwitchedOn(){
        return this.isOn;
    }

    @Override
    public boolean isSwitchedOff(){
        return !this.isOn;
    }
}
