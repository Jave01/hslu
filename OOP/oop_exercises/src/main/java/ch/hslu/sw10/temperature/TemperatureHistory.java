package ch.hslu.sw10.temperature;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.util.*;

public class TemperatureHistory {
    private final Set<Temperature> temps = new HashSet<>();

    private float currentMaxKelvin = 0;

    private float currentMinKelvin = Float.MAX_VALUE;

    private final List<TemperatureChangeListener> tcListeners = new ArrayList<>();

    private static final Logger logger = LogManager.getLogger();

    public boolean add(Temperature t){
        final boolean addedSuccessfully = this.temps.add(t);
        this.handleNewExtremes();
        return addedSuccessfully;
    }

    public boolean remove(Temperature t){
        final boolean removedSuccessfully = this.temps.remove(t);
        this.handleNewExtremes();
        return removedSuccessfully;
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
            for (Temperature temp : this.temps) {
                sum += temp.getDegreeCelsius();
            }
            return sum / count;
        } else {
            return Double.NaN;
        }
    }

    public double getAverageKelvin(){
        return this.getAverageCelsius() + Temperature.kelvinOffset;
    }

    public void addTemperatureChangeListener(TemperatureChangeListener tcListener){
        if (tcListener != null){
            this.tcListeners.add(tcListener);
        }
    }

    public void removeTemperatureChangeListener(TemperatureChangeListener pcListener){
        if (pcListener != null){
            this.tcListeners.remove(pcListener);
        }
    }

    @Override
    public String toString(){
        String s = "Temperature history: ";
        for (Temperature temp : this.temps) {
            s += temp.toString() + ", ";
        }
        return s;
    }

    /**
     * Fires a temperature change event if an extreme changed.
     * Compares the stored max and min value with the new measured max and min values.
     * Calls {@link #fireTemperatureChangeEvent(TemperatureChangeEvent)} if there's a change.
     */
    private void handleNewExtremes(){
        if(this.getCount() > 0) {
            final float max = this.getMaxKelvin();
            final float min = this.getMinKelvin();
            // Check if there's a new max or min
            if (max != this.currentMaxKelvin) {
                this.currentMaxKelvin = max;
                this.fireTemperatureChangeEvent(new TemperatureChangeEvent(this, TemperatureEventType.MAX));
            }
            if (min != this.currentMinKelvin) {
                this.currentMinKelvin = min;
                this.fireTemperatureChangeEvent(new TemperatureChangeEvent(this, TemperatureEventType.MIN));
            }
        }
    }

    /**
     * Calls every listener with the given event.
     * @param evt specification which kind of event (max or min)
     */
    private void fireTemperatureChangeEvent(final TemperatureChangeEvent evt){
        for(TemperatureChangeListener tcListener : this.tcListeners){
            tcListener.temperatureChange(evt);
        }
    }
}
