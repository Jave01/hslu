package ch.hslu.sw05.shapes;

public class Square extends Shape {
    private int length;

    protected Square(int x, int y, int length) {

        super(x, y);
        this.length = length;
    }

    @Override
    public int getPerimeter() {
        return 4 * length;
    }

    @Override
    public int getArea() {
        return length * length;
    }

    public int getLength() {
        return length;
    }

    public void setLength(int length) {
        this.length = length;
    }
}
