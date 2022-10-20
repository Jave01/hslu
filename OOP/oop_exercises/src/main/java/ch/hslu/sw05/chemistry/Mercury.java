package ch.hslu.sw05.chemistry;

public class Mercury extends AbsElement{
    private int pteNumber = 80;
    private int MeltingPointKelvin = 234;
    private int BoilingPointKelvin = 630;

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
