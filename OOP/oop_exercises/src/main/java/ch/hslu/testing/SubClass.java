package ch.hslu.testing;

public class SubClass extends SuperClass{
    private static int lala = 0;

    @Override
    public void jajaja(){
        throw new RuntimeException();
    }
}
