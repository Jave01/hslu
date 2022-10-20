package ch.hslu.sw05.chemistry;

public class Lead extends AbsElement{
    private int pteNumber = 82;
    private int MeltingPointKelvin = 600;
    private int BoilingPointKelvin = 2022;

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
