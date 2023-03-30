#include <stdio.h>
#include <stdbool.h>


char board[9] = "         ";


char checkForWin();
void markBoard(int fieldNumber, char playerChar);
void drawBoard();
int getNumber();

int main(int argc, char const *argv[])
{
    printf("Enter a number between 1 and 9 to set a field\n");

    char playerChars[2] = {'X', 'O'};
    int currentPlayer = 0;
    char winner = 0;

    do
    {
        printf("Player \'%c\' turn:\n> ", playerChars[currentPlayer]);
        markBoard(getNumber()-1, playerChars[currentPlayer]);
        drawBoard();

        winner = checkForWin();
        if(winner != 0){
            printf("Player \'%c\' wins the game\n", winner);
            break;
        }

        currentPlayer = !currentPlayer;

    } while (true);
    
    return 0;
}

char checkForWin(){
    /* Rows */
    for (int i = 0; i < 9; i+=3)
    {
        if (board[i] != ' ' && (board[i] == board[i+1]) && (board[i] == board[i+2])){
            return board[i];
        }
    }

    /* Columns */
    for (int i = 0; i < 3; i++){
        if (board[i] != ' ' && (board[i] == board[i+3]) && (board[i] == board[i+6])){
            return board[i];
        }
    }

    /* Diagonals */
    for (int i = 0; i < 2; i++){
        if (board[0] != ' ' && (board[0] == board[4]) && (board[0] == board[8])){
            return board[0];
        }
        if (board[6] != ' ' && (board[6] == board[4]) && (board[6] == board[2])){
            return board[6];
        }
    }
    return 0;
}

void drawBoard(){
    printf("-------------\n");
    for (int i = 0; i < 9; i+=3)
    {
        printf("| %c | %c | %c |\n", board[i], board[i+1], board[i+2]);
        printf("-------------\n");
    }
}

void markBoard(int fieldNumber, char playerChar){
    board[fieldNumber] = playerChar;
}

int getNumber(){
    int inp = 0;
    do
    {
        scanf("%d", &inp);
        if (inp < 1 || inp > 9)
        {
            printf("inp not valid\n> ");
        } 
        else if (board[inp-1] != ' ')
        {
            printf("field already set\n> ");
        }
        else
        {
            return inp;
        }
    } while (true);
}