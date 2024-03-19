package ch.hslu.sw06.subtyping;

public class Main {
    public static void main(String[] args){
        Shape s1 = new Circle(0, 0, 5);
        Shape s2 = new Rectangle(0, 0, 5, 3);
        s1.move(2, 2);
        s2.move(4, 0);
        System.out.println(((Circle)s1).getDiameter());

    }
}
