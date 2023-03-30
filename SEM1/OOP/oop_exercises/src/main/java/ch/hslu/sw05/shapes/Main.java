package ch.hslu.sw05.shapes;

/**
 * Main class of the {@link ch.hslu.sw05.shapes} package
 */
public class Main {
    public static void main(String args[]){
        Rectangle r = new Rectangle(0, 0, 0, 0);
        Circle s = new Circle(0, 0, 4);

        r.changeDimension(5, 3);
        r.move(3, 2);
//        System.out.println(r.getWidth());
//        System.out.println(r.getHeight());
//        System.out.println(r.getX());
//        System.out.println(r.getY());
//        System.out.println(r.getPerimeter());
//        System.out.println(r.getArea());

        s.setDiameter(5);
        s.move(2, 1);
//        System.out.println(s.getDiameter());
//        System.out.println(s.getX());
//        System.out.println(s.getY());
//        System.out.println(s.getPerimeter());
//        System.out.println(s.getArea());


    }
}
