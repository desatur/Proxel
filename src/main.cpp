#include <iostream>
#include "Log.hpp"

int main(int argc, char** argv) {
    Log::Init();
    for (size_t i = 0; i < 10000000000; i++)
    {
        LOG_INFO("TEST");
    }
    
    return 0;
}
