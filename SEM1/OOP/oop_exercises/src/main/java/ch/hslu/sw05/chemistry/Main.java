package ch.hslu.sw05.chemistry;

/**
 * Main class of the {@link ch.hslu.sw05.chemistry} package
 */
public class Main {
    public static void main(String[] args) {
        // Element Task
        Temperature temp = new Temperature(0, TemperatureType.KELVIN);

        Element h = new Element(273, 373, 1, "hydrogen");
        Lead pb = new Lead();
        Nitrogen n = new Nitrogen();
//        Mercury hg = new Mercury();

        System.out.println("--- Temperature: 0 kelvin ---");
        System.out.println("hydrogen: " + temp.getAggregateState(h));
        System.out.println("lead: " + temp.getAggregateState(pb));
        System.out.println("nitrogen: " + n.getAggregateState(temp.getKelvin()));

//        temp.addKelvin(600);
//
//        System.out.println("\n--- Temperature: 600 kelvin ---");
//        System.out.println("Aggregate state hydrogen: " + temp.getAggregateState(h));
//        System.out.println("Aggregate state lead: " + temp.getAggregateState(pb));
//        System.out.println("Aggregate state nitrogen: " + n.getAggregateState(temp.getKelvin()));
//
//        temp.addKelvin(2000);
//
//        System.out.println("\n--- Temperature: 2600 kelvin ---");
//        System.out.println("Aggregate state hydrogen: " + temp.getAggregateState(h));
//        System.out.println("Aggregate state lead: " + temp.getAggregateState(pb));
//        System.out.println("Aggregate state nitrogen: " + n.getAggregateState(temp.getKelvin()));
    }
}