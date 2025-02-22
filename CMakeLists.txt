cmake_minimum_required(VERSION 3.10.0)
project(Proxel VERSION 0.1.0 LANGUAGES C CXX)

file (GLOB_RECURSE Proxel_SOURCES CONFIGURE_DEPENDS "src/*.cpp")
file (GLOB_RECURSE Proxel_HEADERS CONFIGURE_DEPENDS "src/*.hpp")

set (Proxel_INCLUDE_DIRS "")

foreach (_headerFile ${Proxel_HEADERS})
    get_filename_component(_dir ${_headerFile} PATH)
    list (APPEND Proxel_INCLUDE_DIRS ${_dir})
endforeach()

list (REMOVE_DUPLICATES Proxel_INCLUDE_DIRS)

add_executable(${PROJECT_NAME} ${Proxel_SOURCES})