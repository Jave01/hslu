CC = gcc
SRC_DIR = src
OUT_DIR = out
SRCS = $(wildcard $(SRC_DIR)/*.c)
OBJS = $(patsubst $(SRC_DIR)/%.c,$(OUT_DIR)/%.o,$(SRCS))
HEADER_DIR = inc

all: pw_manager

pw_manager: $(OBJS)
	$(CC) $(CFLAGS) -I$(HEADER_DIR) -o $@ $^

$(OUT_DIR)/%.o: $(SRC_DIR)/%.c
	$(CC) $(CFLAGS) -I$(HEADER_DIR) -c $< -o $@

clean:
	rm -rf $(OUT_DIR)/*.o myprogram

.PHONY: clean
