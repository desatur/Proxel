#include "Log.hpp"

std::shared_ptr<spdlog::logger> Log::s_CoreLogger;

void Log::Init() {
    spdlog::set_pattern("%^[%T.%e] [%n] [%l] >> %v%$");

    s_CoreLogger = spdlog::stdout_color_mt("Proxel");
    s_CoreLogger->set_level(spdlog::level::trace);
}