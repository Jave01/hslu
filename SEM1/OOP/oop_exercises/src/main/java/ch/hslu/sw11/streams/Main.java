package ch.hslu.sw11.streams;

import java.io.DataInputStream;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

public class Main {
    public static void main(String[] args) {
//        final String fPath = "./src/main/java/ch/hslu/sw11/test.txt";
        final String fPath = ".\\src\\main\\java\\ch\\hslu\\sw11\\test.txt";
        try(DataInputStream dis = new DataInputStream(new FileInputStream(fPath))) {
            System.out.println(dis.readDouble());
        } catch (FileNotFoundException e) {
            throw new RuntimeException(e);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }
}
