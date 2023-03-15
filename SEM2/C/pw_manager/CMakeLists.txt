cmake_minimum_required(VERSION 3.24)
project(pw_manager C)

set(CMAKE_C_STANDARD 11)

include_directories(include)

#add_executable(pw_manager main.c include/main.h file_handler.c include/file_handler.h)
add_executable(pw_manager file_handler.c include/file_handler.h)
