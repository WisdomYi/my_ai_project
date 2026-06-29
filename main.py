"""迷你游戏合集 - 主启动器"""
import pygame
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from games.billiards import BilliardsGame
from games.pinball import PinballGame
from games.pokemon import PokemonGame

SCREEN_W, SCREEN_H = 900, 650
BG_COLOR = (20, 20, 40)
CARD_COLOR = (35, 35, 65)
CARD_HOVER = (55, 55, 95)
ACCENT = (100, 180, 255)
TEXT_COLOR = (230, 230, 240)
SUB_COLOR = (160, 170, 190)


class GameLauncher:
    """游戏合集主菜单"""

    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((SCREEN_W, SCREEN_H))
        pygame.display.set_caption("Mini Game Collection - 迷你游戏合集")
        self.clock = pygame.time.Clock()
        self.font_title = pygame.font.Font(None, 56)
        self.font_name = pygame.font.Font(None, 32)
        self.font_desc = pygame.font.Font(None, 20)
        self.font_small = pygame.font.Font(None, 18)

        self.games = [
            {
                "name": "Billiards - 桌球大师",
                "desc": "Classic pool physics game\nUse the cue to pocket all colored balls",
                "cls": BilliardsGame,
                "icon_color": (50, 180, 80),
            },
            {
                "name": "Pinball - 弹珠台",
                "desc": "Classic arcade pinball\nUse flippers to keep the ball alive",
                "cls": PinballGame,
                "icon_color": (220, 140, 40),
            },
            {
                "name": "Pokemon Arena - 宝可梦竞技场",
                "desc": "Turn-based Pokemon battle\nChoose moves and defeat your opponent",
                "cls": PokemonGame,
                "icon_color": (220, 60, 60),
            },
        ]

    def draw_card(self, x, y, w, h, game, hover):
        color = CARD_HOVER if hover else CARD_COLOR
        rect = pygame.Rect(x, y, w, h)
        pygame.draw.rect(self.screen, color, rect, border_radius=16)
        pygame.draw.rect(self.screen, ACCENT, rect, 2, border_radius=16)

        cx, cy = x + 50, y + h // 2
        pygame.draw.circle(self.screen, game["icon_color"], (cx, cy), 28)
        pygame.draw.circle(self.screen, (255, 255, 255, 60), (cx, cy), 28, 2)

        name_surf = self.font_name.render(game["name"], True, TEXT_COLOR)
        self.screen.blit(name_surf, (x + 95, y + 20))

        for i, line in enumerate(game["desc"].split("\n")):
            desc_surf = self.font_desc.render(line, True, SUB_COLOR)
            self.screen.blit(desc_surf, (x + 95, y + 58 + i * 22))

    def run(self):
        running = True
        while running:
            mx, my = pygame.mouse.get_pos()
            self.screen.fill(BG_COLOR)

            title = self.font_title.render("Mini Game Collection", True, ACCENT)
            self.screen.blit(title, (SCREEN_W // 2 - title.get_width() // 2, 40))

            subtitle = self.font_desc.render("Choose a game to start!", True, SUB_COLOR)
            self.screen.blit(subtitle, (SCREEN_W // 2 - subtitle.get_width() // 2, 100))

            card_w, card_h = 580, 130
            start_x = SCREEN_W // 2 - card_w // 2

            for i, game in enumerate(self.games):
                card_y = 160 + i * (card_h + 20)
                rect = pygame.Rect(start_x, card_y, card_w, card_h)
                hover = rect.collidepoint(mx, my)
                self.draw_card(start_x, card_y, card_w, card_h, game, hover)

            footer = self.font_small.render(
                "ESC to quit | Click a card to play", True, (100, 100, 140)
            )
            self.screen.blit(footer, (SCREEN_W // 2 - footer.get_width() // 2, SCREEN_H - 40))

            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                if event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE:
                    running = False
                if event.type == pygame.MOUSEBUTTONDOWN:
                    for i, game in enumerate(self.games):
                        rect = pygame.Rect(start_x, 160 + i * (card_h + 20), card_w, card_h)
                        if rect.collidepoint(mx, my):
                            game_instance = game["cls"](self.screen, self.clock)
                            game_instance.run()
                            pygame.display.set_mode((SCREEN_W, SCREEN_H))
                            pygame.display.set_caption("Mini Game Collection - 迷你游戏合集")

            pygame.display.flip()
            self.clock.tick(60)

        pygame.quit()
        sys.exit()


if __name__ == "__main__":
    GameLauncher().run()
