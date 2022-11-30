package ch.hslu.sw10.temperature;

import java.util.EventObject;
import java.util.Objects;

public abstract class TemperatureChangeEvent extends EventObject {

    public final TemperatureEventType tempEventType;

    public TemperatureChangeEvent(Object source, TemperatureEventType tempEventType) {
        super(source);
        this.tempEventType = tempEventType;
    }

    public abstract void propertyChange(TemperatureChangeEvent evt);

}
