package ch.hslu.sw06.switchables;

/**
 * Class for objects who need to count their switching cycles.
 */
public class CountingSwitchable implements Switchable , Named{
    private boolean isOn = false;
    private String name = "";
    private long switchCycles = 0;

    /**
     * Turn switch off
     */
    @Override
    public void switchOn() {
        this.isOn = true;
    }

    /**
     * Turn switch on.
     */
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

    public long getSwitchCycles() {
        return switchCycles;
    }
}
