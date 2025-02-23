#include <iostream>
#include "Log.hpp"
#include "ConfigHandler.hpp"

int main(int argc, char** argv) {
    Log::Init();
    LOG_INFO("Starting Proxel " PROXEL_VERSION);
    ConfigHandler::GetInstance().Load();
    return 0;
}
