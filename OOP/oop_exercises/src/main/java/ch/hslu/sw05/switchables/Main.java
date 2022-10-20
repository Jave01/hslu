package ch.hslu.sw05.switchables;

public class Main {
    public static void main(String[] args){
        CountingSwitchable cs = new CountingSwitchable();

        cs.switchOn();
        cs.switchOff();
        cs.switchOn();
        cs.switchOff();
        cs.switchOff();
        cs.switchOff();
        System.out.println(cs.getSwitchCycles());
    }
}
