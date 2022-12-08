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

    /**
     * Add a temperature to the history.
     * @param t a Temperature object.
     * @return indicator if the addition was successful.
     */
    public boolean add(Temperature t){
        final boolean addedSuccessfully = this.temps.add(t);
        this.handleNewExtremes();
        return addedSuccessfully;
    }

    /**
     * Remove a temperature from the history.
     * If there is a temperature in the history which .equals() Method returns true, it will be removed.
     * @param t a Temperature object.
     * @return indicator if the removal was successful.
     */
    public boolean remove(Temperature t){
        final boolean removedSuccessfully = this.temps.remove(t);
        this.handleNewExtremes();
        return removedSuccessfully;
    }

    /**
     * Return number of elements in the history.
     * @return number of elements in history.
     */
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

    /**
     * Returns the average temperature value in degree Celsius.
     * If there are no elements in the history in returns Float.NaN.
     * @return average temperature in degree Celsius.
     */
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

    /**
     * Add a listener for all the implemented class events.
     * @param tcListener the listener to add.
     * @return indicator if the addition was successful.
     */
    public boolean addTemperatureChangeListener(TemperatureChangeListener tcListener){
        if (tcListener != null){
            return this.tcListeners.add(tcListener);
        }
        return false;
    }

    /**
     * Remove a listener.
     * @param pcListener the listener which will be removed.
     * @return indicator if removal was successful.
     */
    public boolean removeTemperatureChangeListener(TemperatureChangeListener pcListener){
        if (pcListener != null){
            return this.tcListeners.remove(pcListener);
        }
        return false;
    }

    /**
     * Returns a concatenation of the string values from each temperature in history.
     * @return all the temperature string values concatenated.
     */
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
                this.fireTemperatureChangeEvent(new TemperatureChangeEvent(this, TemperatureEventType.MAX, max));
            }
            if (min != this.currentMinKelvin) {
                this.currentMinKelvin = min;
                this.fireTemperatureChangeEvent(new TemperatureChangeEvent(this, TemperatureEventType.MIN, min));
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
