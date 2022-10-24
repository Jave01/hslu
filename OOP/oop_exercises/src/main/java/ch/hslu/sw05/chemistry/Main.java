package ch.hslu.sw05.chemistry;

public class Main {
    public static void main(String[] args) {
        // Element Task
        Temperature temp = new Temperature(0, TemperatureType.KELVIN);
        Element h = new Element(273, 373, 1, "hydrogen");
//
//        System.out.println(temp.getAggregateState(h));
//        temp.addKelvin(300);
//        System.out.println(temp.getAggregateState(h));
//        temp.addKelvin(100);
//        System.out.println(temp.getAggregateState(h));
//
//        Nitrogen n = new Nitrogen();
//
        temp.getAggregateState(h);
    }
}
