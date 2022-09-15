// Author: Rebecca Bushko

using SDL2;

namespace RebeccaBushko.GameOfLife
{
    public class GuiSimulation
    {
        // Number of max days to simulate. Change if you would like to exceed this.
        private const int MAX_SIM_STEPS = 100;

        // Milliseconds between sim frames in play mode.
        private const int SIM_TIME_MS = 200;

        // Window dimensions & UI constants
        private const int WINDOW_WIDTH = 504;
        private const int WINDOW_HEIGHT = 615;
        private const int MARGIN_LEFT = 3;
        private const int MARGIN_TOP = 115;

        private const int GRID_BOX_SIZE = 10;
        private const int GRID_BOX_BORDER = 1;

        private const int FONT_SIZE = 16;
        private const int FONT_SIZE_LARGE = 22;

        // Assets
        private const string LOGO_IMAGE = "Assets/valorant-logo.png";
        private const string FONT_PATH = "Assets/Valorant Font.ttf";

        private IntPtr window;
        private IntPtr renderer;

        private IntPtr font;
        private SDL.SDL_Color colorRed;
        private SDL.SDL_Color colorWhite;
        private SDL.SDL_Color colorBlack;

        private IntPtr tex;
        private SDL.SDL_Rect sourceRect;

        private QuadTreeGameOfLife simulation;

        // Variables for running the updates
        private bool running = true;
        private bool simulate = false;
        private bool draw = true;

        private bool playMode = false;
        private int simSteps = 0;

        public GuiSimulation()
        {
            // SDL initialization for graphics
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"Error initializing SDL: {SDL.SDL_GetError()}");
            }

            window = SDL.SDL_CreateWindow(
                "Game of Life Simulation - Rebecca Bushko",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                WINDOW_WIDTH,
                WINDOW_HEIGHT,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"Error creating window: {SDL.SDL_GetError()}");
            }

            renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"Error creating renderer: {SDL.SDL_GetError()}");
            }

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"Error initializing SDL image: {SDL_image.IMG_GetError()}");
            }

            if (SDL_ttf.TTF_Init() == -1)
            {
                Console.WriteLine($"Error initializing SDL fonts: {SDL_ttf.TTF_GetError()}");
            }

            font = SDL_ttf.TTF_OpenFont(FONT_PATH, FONT_SIZE);
            colorRed = new SDL.SDL_Color();
            colorRed.r = 250;
            colorRed.g = 68;
            colorRed.b = 84;

            colorWhite = new SDL.SDL_Color();
            colorWhite.r = 255;
            colorWhite.g = 255;
            colorWhite.b = 255;

            colorBlack = new SDL.SDL_Color();
            colorBlack.r = 0;
            colorBlack.g = 0;
            colorBlack.b = 0;

            sourceRect = new SDL.SDL_Rect() { x = 0, y = 0, w = GRID_BOX_SIZE, h = GRID_BOX_SIZE };

            IntPtr surface = SDL_image.IMG_Load(LOGO_IMAGE);
            tex = SDL.SDL_CreateTextureFromSurface(renderer, surface);
            SDL.SDL_FreeSurface(surface);

            simulation = new QuadTreeGameOfLife(new List<Tuple<long, long>>());
        }

        public bool IsRunning()
        {
            return running;
        }

        public void Update()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    running = false;
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    switch (e.key.keysym.sym)
                    {
                        // Simulate one step if not in play mode
                        case SDL.SDL_Keycode.SDLK_SPACE:
                            if (!playMode)
                            {
                                simulate = true;
                                draw = true;
                            }
                            break;
                        // Toggle play mode on
                        case SDL.SDL_Keycode.SDLK_RETURN:
                            playMode = !playMode;
                            break;
                        // Remove 1 step from the sim steps
                        case SDL.SDL_Keycode.SDLK_LEFT:
                            simSteps -= 1;
                            simSteps = Math.Max(0, simSteps);
                            draw = true;
                            break;
                        // Remove 5 steps from sim steps
                        case SDL.SDL_Keycode.SDLK_DOWN:
                            simSteps -= 5;
                            simSteps = Math.Max(0, simSteps);
                            draw = true;
                            break;
                        // Add 5 steps to sim steps
                        case SDL.SDL_Keycode.SDLK_UP:
                            simSteps += 5;
                            simSteps = Math.Min(MAX_SIM_STEPS, simSteps);
                            draw = true;
                            break;
                        // Add 1 step to sim steps
                        case SDL.SDL_Keycode.SDLK_RIGHT:
                            simSteps += 1;
                            simSteps = Math.Min(MAX_SIM_STEPS, simSteps);
                            draw = true;
                            break;
                    }
                }
                else if (!playMode && e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
                {
                    SDL.SDL_GetMouseState(out int x, out int y);
                    OnClick(x, y);
                    draw = true;
                }
            }

            if (playMode)
            {
                if (simSteps <= 0)
                {
                    playMode = false;
                }
                else
                {
                    SDL.SDL_Delay(SIM_TIME_MS);
                    simulate = true;

                    simSteps--;
                }
                draw = true;
            }

            if (simulate)
            {
                simulation.Simulate();
                simulate = false;
            }

            if (draw)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 50, 50, 50, 255);
                SDL.SDL_RenderClear(renderer);

                DrawInstructions();
                DrawGrid();

                SDL.SDL_RenderPresent(renderer);
                draw = false;
            }
        }

        private void DrawGrid()
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            for (int x = 0; x < WINDOW_WIDTH / GRID_BOX_SIZE; x++)
            {
                for (int y = 0; y < WINDOW_WIDTH / GRID_BOX_SIZE; y++)
                {
                    DrawBox(y, x);
                }
            }

            foreach (Cell cell in simulation.GetCells())
            {
                if (cell.IsAlive)
                {
                    DrawAlive((int) cell.Y, (int) cell.X);
                }
            }
        }

        private void DrawAlive(int x, int y)
        {
            DrawBox(x, y);
            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = MARGIN_LEFT + x * GRID_BOX_SIZE,
                y = MARGIN_TOP + y * GRID_BOX_SIZE,
                w = GRID_BOX_SIZE,
                h = GRID_BOX_SIZE
            };

            SDL.SDL_RenderCopy(renderer, tex, ref sourceRect, ref rect);
        }

        private void DrawBox(int x, int y)
        {
            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = MARGIN_LEFT + x * GRID_BOX_SIZE,
                y = MARGIN_TOP + y * GRID_BOX_SIZE,
                w = GRID_BOX_SIZE - GRID_BOX_BORDER,
                h = GRID_BOX_SIZE - GRID_BOX_BORDER
            };

            // Draw a filled in rectangle.
            SDL.SDL_RenderFillRect(renderer, ref rect);
        }

        // Draw the instructions at the top of the window
        private void DrawInstructions()
        {
            // Help text
            SDL_ttf.TTF_SetFontSize(font, FONT_SIZE);
            SDL.SDL_Rect srcRect = new SDL.SDL_Rect() { x = 0, y = 0, w = WINDOW_WIDTH, h = MARGIN_TOP - GRID_BOX_BORDER };
            IntPtr fontSurface = SDL_ttf.TTF_RenderText_Shaded_Wrapped(
                font,
                "Toggle cells ON/OFF by clicking the grid.\nAdjust the number of simulation steps using the arrow keys.\nUse the 'Enter' key to toggle the simulation.\nPress 'Space' to run a single step of the simulation.",
                colorRed,
                colorBlack,
                WINDOW_WIDTH);

            IntPtr fontTex = SDL.SDL_CreateTextureFromSurface(renderer, fontSurface);
            SDL.SDL_QueryTexture(fontTex, out _, out _, out int w, out int h);
            SDL.SDL_Rect destRect = new SDL.SDL_Rect() { x = 0, y = 0, w = w, h = h };
            SDL.SDL_RenderCopy(renderer, fontTex, ref srcRect, ref destRect);
            SDL.SDL_FreeSurface(fontSurface);

            SDL_ttf.TTF_SetFontSize(font, FONT_SIZE_LARGE);
            // Steps to simulate
            fontSurface = SDL_ttf.TTF_RenderText_Shaded(
                font,
                playMode ? $"  Simulating: {simSteps}  " : $"  Steps to Simulate: {simSteps}  ",
                colorWhite,
                colorBlack);

            fontTex = SDL.SDL_CreateTextureFromSurface(renderer, fontSurface);
            SDL.SDL_QueryTexture(fontTex, out _, out _, out w, out h);
            destRect = new SDL.SDL_Rect() { x = WINDOW_WIDTH / 2 - w / 2, y = MARGIN_TOP - h - GRID_BOX_BORDER * 2, w = w, h = h };
            SDL.SDL_RenderCopy(renderer, fontTex, ref srcRect, ref destRect);
            SDL.SDL_FreeSurface(fontSurface);
        }

        private void OnClick(int screenX, int screenY)
        {
            if (screenY < MARGIN_TOP)
            {
                return;
            }

            int gridX = (screenX - MARGIN_LEFT) / GRID_BOX_SIZE;
            int gridY = (screenY - MARGIN_TOP) / GRID_BOX_SIZE;

            simulation.TogglePoint((ulong) gridY, (ulong) gridX);
        }

        // Cleanup SDL
        public void Destroy()
        {
            SDL_ttf.TTF_CloseFont(font);
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
