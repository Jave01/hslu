package ch.hslu.sw05.switchables;

public class CountingSwitchable implements Switchable , Named{
    private boolean isOn = false;
    private String name = "";
    private long switchCycles = 0;

    public long getSwitchCycles() {
        return switchCycles;
    }

    @Override
    public void switchOn() {
        this.isOn = true;
    }

    @Override
    public void switchOff() {
        if(this.isOn){
            this.switchCycles += 1;
        }
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

    @Override
    public void setName(String name) {
        this.name = name;
    }

    @Override
    public String getName() {
        return this.name;
    }
}
