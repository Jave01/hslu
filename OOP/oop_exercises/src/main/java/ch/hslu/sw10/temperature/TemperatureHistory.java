package ch.hslu.sw10.temperature;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.beans.PropertyChangeListener;
import java.util.*;

public class TemperatureHistory {
    private Set<Temperature> temps = new HashSet<>();

    private final List<PropertyChangeListener> pcListeners = new ArrayList<>();

    private static final Logger logger = LogManager.getLogger();

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

    public float getMinCelsius(){
        return Collections.min(this.temps).getDegreeCelsius();
    }

    public float getMinFahrenheit(){
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

    public double getAverageKelvin(){
        return this.getAverageCelsius() + Temperature.kelvinOffset;
    }

    public double getAverageWithoutZeroCheck(){
        double sum = 0;
        Iterator<Temperature> i = this.temps.iterator();
        while(i.hasNext()){
            sum += i.next().getDegreeCelsius();
        }
        return sum / 0;//this.getCount();
    }

    public void propertyChange(TemperatureChangeEvent evt) {
        if (evt.tempEventType == TemperatureEventType.MAX){
            this.logger.info("New max value in history: " + this.getMaxKelvin() + " K");
        } else if (evt.tempEventType == TemperatureEventType.MIN){
            this.logger.info("New min value in history: " + this.getMinKelvin() + " K");
        }
    }

    public void addPropertyChangeListener(PropertyChangeListener pcListener){
        if (pcListener != null){
            this.pcListeners.add(pcListener);
        }
    }

    public void removePropertyChangeListener(PropertyChangeListener pcListener){
        if (pcListener != null){
            this.pcListeners.remove(pcListener);
        }
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
