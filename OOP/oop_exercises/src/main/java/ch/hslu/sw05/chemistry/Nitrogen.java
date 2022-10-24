package ch.hslu.sw05.chemistry;

public class Nitrogen extends AbsElement{
    private int pteNumber = 7;
    private int MeltingPointKelvin = 63;
    private int BoilingPointKelvin = 77;

    public int getPteNumber() {
        return pteNumber;
    }

    public int getMeltingPointKelvin() {
        return MeltingPointKelvin;
    }

    public int getBoilingPointKelvin() {
        return BoilingPointKelvin;
    }

    @Override
    public AggregateState getAggregateState(Temperature tempClass) {
        return null;
    }
}
