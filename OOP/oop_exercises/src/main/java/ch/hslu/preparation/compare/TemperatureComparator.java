package ch.hslu.preparation.compare;

import java.util.Comparator;

public class TemperatureComparator implements Comparator<Temperature> {
    @Override
    public int compare(Temperature o1, Temperature o2) {
        return Float.compare(o1.getDegreeCelsius(), o2.getDegreeCelsius());
    }
}
