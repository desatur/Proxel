#include <iostream>
#include "Log.hpp"
#include "Config.hpp"

int main(int argc, char** argv) {
    Log::Init();
    Config& config = Config::GetInstance();
    return 0;
}
