#pragma once
#include <memory>
#include <yaml-cpp/yaml.h>
#include <string>
#include <fstream>
#include <iostream>

class ConfigHandler {
public:
    void Load();
    void Save();
    static ConfigHandler& GetInstance();
    std::shared_ptr<YAML::Node> GetConfigNode();
    
private:
    void CreateDefaultConfig();
    static std::unique_ptr<ConfigHandler> instance;
    std::shared_ptr<YAML::Node> m_node;
};