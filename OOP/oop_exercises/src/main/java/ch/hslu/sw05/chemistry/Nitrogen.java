package ch.hslu.sw05.chemistry;

public class Nitrogen extends AbsElement{
    private int pteNumber = 7;
    private int MeltingPointKelvin = 63;
    private int BoilingPointKelvin = 77;

    @Override
    public int getPteNumber() {
        return pteNumber;
    }

    @Override
    public int getMeltingPointKelvin() {
        return MeltingPointKelvin;
    }

    @Override
    public int getBoilingPointKelvin() {
        return BoilingPointKelvin;
    }
}
