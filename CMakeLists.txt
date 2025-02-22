cmake_minimum_required(VERSION 3.10.0)
project(Proxel VERSION 0.1.0 LANGUAGES C CXX)

# spdlog

set(SPDLOG_VERSION 1.15.1)

include(FetchContent)

FetchContent_Declare(
	spdlog
	URL https://github.com/gabime/spdlog/archive/refs/tags/v${SPDLOG_VERSION}.tar.gz
	FIND_PACKAGE_ARGS ${SPDLOG_VERSION}
    DOWNLOAD_EXTRACT_TIMESTAMP true
)

FetchContent_MakeAvailable(spdlog)

file (GLOB_RECURSE Proxel_SOURCES CONFIGURE_DEPENDS "src/*.cpp")
file (GLOB_RECURSE Proxel_HEADERS CONFIGURE_DEPENDS "src/*.hpp")

set (Proxel_INCLUDE_DIRS "")

foreach (_headerFile ${Proxel_HEADERS})
    get_filename_component(_dir ${_headerFile} PATH)
    list (APPEND Proxel_INCLUDE_DIRS ${_dir})
endforeach()

list (REMOVE_DUPLICATES Proxel_INCLUDE_DIRS)

add_executable(${PROJECT_NAME} ${Proxel_SOURCES})

target_link_libraries(${PROJECT_NAME} spdlog::spdlog)