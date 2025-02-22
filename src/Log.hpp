#pragma once
#include <memory>
#include <spdlog/spdlog.h>
#include <spdlog/sinks/stdout_color_sinks.h>

class Log {
public:
    static void Init();
    inline static std::shared_ptr<spdlog::logger>& GetCoreLogger() { return s_CoreLogger; }

private:
    static std::shared_ptr<spdlog::logger> s_CoreLogger;
};

// Macros
#define LOG_ERROR(...) Log::GetCoreLogger()->error(__VA_ARGS__)
#define LOG_WARN(...) Log::GetCoreLogger()->warn(__VA_ARGS__)
#define LOG_INFO(...) Log::GetCoreLogger()->info(__VA_ARGS__)
#define LOG_TRACE(...) Log::GetCoreLogger()->trace(__VA_ARGS__)
#define LOG_DEBUG(...) Log::GetCoreLogger()->debug(__VA_ARGS__)
#define LOG_FATAL(...) Log::GetCoreLogger()->fatal(__VA_ARGS__)