package ch.hslu.sw11;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.io.*;
import java.nio.charset.Charset;
import java.util.Scanner;

public class Main {
    private final static Logger logger = LogManager.getLogger();

    private static Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {
        // System.out.println(System.getProperty("user.dir"));
        final String fPath = "./src/main/java/ch/hslu/sw11/test.txt";
        String input = scanner.nextLine();

        Main.write(fPath, input);
        Main.read(fPath);
    }

    public static boolean read(String path) {
        try (BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path)))) {
            String line = br.readLine();
            while (line != null) {
                System.out.println(line);
                line = br.readLine();
            }
            return true;
        } catch (IOException e) {
            logger.error(e);
            return false;
        }
    }

    public static boolean write(String path, String content) {
        try (PrintWriter pw = new PrintWriter(new BufferedWriter(new OutputStreamWriter(
                new FileOutputStream(path), Charset.forName("UTF-8"))));) {
            pw.write(content);
            return true;
        } catch (IOException e) {
            logger.error(e);
            return false;
        }
    }
}
