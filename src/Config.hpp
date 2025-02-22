#pragma once
#include <memory>
#include <yaml-cpp/yaml.h>
#include <string>

class Config {
public:
    Config(const std::string& filename = "config.yaml");
    void Load();
    void Save();
    std::shared_ptr<YAML::Node> GetConfigNode();
    static Config& GetInstance();

private:
    void CreateDefaultConfig();
    std::string m_filename;
    std::shared_ptr<YAML::Node> m_node;
};