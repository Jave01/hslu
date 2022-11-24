package ch.hslu.sw08.collections;

import ch.hslu.sw08.access_modifier.Temperature;

import java.util.Collections;
import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

public class TemperatureHistory {
    private Set<Temperature> temps = new HashSet<>();

    public boolean add(Temperature t){
        return this.temps.add(t);
    }

    public boolean remove(Temperature t){
        return this.temps.remove(t);
    }

    public int getCount(){
        return this.temps.size();
    }

    public float getMinKelvin(){
        return Collections.min(this.temps).getKelvin();
    }

    public float minCelsius(){
        return Collections.min(this.temps).getDegreeCelsius();
    }

    public float minFahrenheit(){
        return Collections.min(this.temps).getFahrenheit();
    }

    public float getMaxKelvin(){
        return Collections.max(this.temps).getKelvin();
    }

    public float getMaxCelsius(){
        return Collections.max(this.temps).getDegreeCelsius();
    }

    public float getMaxFahrenheit(){
        return Collections.max(this.temps).getFahrenheit();
    }

    public double getAverageCelsius(){
        final int count = this.getCount();
        if (count != 0) {
            double sum = 0;
            Iterator<Temperature> i = this.temps.iterator();
            while(i.hasNext()){
                sum += i.next().getDegreeCelsius();
            }
            return sum / count;
        } else {
            return Double.NaN;
        }
    }


    public double getAverageWithoutZeroCheck(){
        double sum = 0;
        Iterator<Temperature> i = this.temps.iterator();
        while(i.hasNext()){
            sum += i.next().getDegreeCelsius();
        }
        return sum / 0;//this.getCount();
    }

    @Override
    public String toString(){
        String s = "Temperature history: ";
        Iterator<Temperature> i = this.temps.iterator();
        while (i.hasNext()){
            s += i.next().toString() + ", ";
        }
        return s;
    }
}
