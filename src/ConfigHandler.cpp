#include "ConfigHandler.hpp"
#define FILENAME "proxel.yml"

std::unique_ptr<ConfigHandler> ConfigHandler::instance = nullptr;

void ConfigHandler::Load() {
    std::ifstream file(FILENAME);
    if (!file.good()) {
        CreateDefaultConfig();
    }
    m_node = std::make_shared<YAML::Node>(YAML::LoadFile(FILENAME));
}

void ConfigHandler::Save() {
    std::ofstream fout(FILENAME);
    fout << *m_node;
}

std::shared_ptr<YAML::Node> ConfigHandler::GetConfigNode() { return m_node; }

ConfigHandler& ConfigHandler::GetInstance() {
    if (!instance) {
        instance = std::make_unique<ConfigHandler>();
    }
    return *instance;
}

void ConfigHandler::CreateDefaultConfig() {
    YAML::Node cfg;
    YAML::Node endpoints;

    auto createEndpoint = [](const std::string& name, const std::string& ip, int port, bool enabled) {
        YAML::Node endpoint;
        endpoint["name"] = name;
        endpoint["ip"] = ip;
        endpoint["port"] = port;
        endpoint["enabled"] = enabled;
        return endpoint;
    };

    endpoints.push_back(createEndpoint("localhost", "localhost", 25565, true));
    endpoints.push_back(createEndpoint("Main", "0.0.0.0", 25565, false));

    cfg["endpoints"] = endpoints;
    std::ofstream fout(FILENAME);
    if (!fout) {
        return;
    }
    fout << cfg;
}