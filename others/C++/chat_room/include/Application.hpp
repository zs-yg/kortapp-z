#ifndef APPLICATION_HPP
#define APPLICATION_HPP

class MainWindow;

class Application {
public:
    Application(int argc, char** argv);
    ~Application();
    
    int run();

private:
    MainWindow* mainWindow;
};

#endif // APPLICATION_HPP
