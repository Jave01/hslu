package ch.hslu.sw04;
public class Main {
    /**
     * @param args
     */
    public static void main(String[] args) {
        Motor m = new Motor();

        System.out.println("Is on: " + m.isSwitchedOn());
        System.out.println("Is off: " + m.isSwitchedOff());
        m.switchOn();
        System.out.println("Is on: " + m.isSwitchedOn());
        System.out.println("Is off: " + m.isSwitchedOff());
        m.switchOff();
        System.out.println("Is on: " + m.isSwitchedOn());
        System.out.println("Is off: " + m.isSwitchedOff());
    }
}