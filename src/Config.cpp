#include "Config.hpp"
#include <fstream>
#include <iostream>

Config::Config(const std::string& filename) : m_filename(filename) {
    Load();
}

void Config::Load() {
    std::ifstream file(m_filename);
    if (!file.good()) {
        CreateDefaultConfig();
    }
    m_node = std::make_shared<YAML::Node>(YAML::LoadFile(m_filename));
}

void Config::Save() {
    std::ofstream fout(m_filename);
    fout << *m_node;
}

std::shared_ptr<YAML::Node> Config::GetConfigNode() { return m_node; }

Config& Config::GetInstance() {
    static Config instance;
    return instance;
}

#include <yaml-cpp/yaml.h>
#include <fstream>

void Config::CreateDefaultConfig() {
    YAML::Node cfg;
    YAML::Node endpoints;
    
    YAML::Node endpoint1;
    endpoint1["name"] = "localhost";
    endpoint1["ip"] = "localhost";
    endpoint1["port"] = 25565;
    endpoint1["enabled"] = true;
    endpoints.push_back(endpoint1);
    
    YAML::Node endpoint2;
    endpoint2["name"] = "Main";
    endpoint2["ip"] = "0.0.0.0";
    endpoint2["port"] = 25565;
    endpoint2["enabled"] = false;
    endpoints.push_back(endpoint2);
    
    cfg["endpoints"] = endpoints;
    
    std::ofstream fout(m_filename);
    fout << cfg;
}
