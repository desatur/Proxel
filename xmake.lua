add_rules("mode.debug", "mode.release")

target("Proxel")
    set_kind("binary") -- Proxel is a binary.
    set_languages("c++20") -- Set language to C++20.
    set_toolchains("mingw") -- Best toolchain for this project.
    add_files("src/*.cpp") -- Add all .cpp sources.
    add_includedirs("include") -- Add all .hpp headers.
    set_optimize("fastest") -- Optimize builds.