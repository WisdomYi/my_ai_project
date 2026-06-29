"""宝可梦竞技场 - 回合制对战游戏"""
import pygame
import random
import math

SCREEN_W, SCREEN_H = 900, 600
BG_COLOR = (30, 40, 60)
PANEL_COLOR = (25, 30, 50)
HEALTH_GREEN = (40, 200, 40)
HEALTH_YELLOW = (220, 200, 20)
HEALTH_RED = (220, 40, 40)
BUTTON_COLOR = (50, 60, 100)
BUTTON_HOVER = (80, 90, 150)
TEXT_COLOR = (230, 235, 250)

# Pokemon data
POKEMON = {
    "Pikachu": {
        "hp": 120, "atk": 55, "def": 40,
        "moves": [
            ("Thunder Shock", 40, "Thunderbolt attack!", (255, 255, 0)),
            ("Quick Attack", 30, "Swift strike!", (200, 200, 200)),
            ("Iron Tail", 50, "Steel-hard tail whip!", (180, 180, 200)),
            ("Thunder", 70, "Massive lightning strike!", (255, 220, 0)),
        ],
        "color": (255, 220, 50),
    },
    "Charizard": {
        "hp": 140, "atk": 65, "def": 45,
        "moves": [
            ("Flamethrower", 50, "Scorching fire breath!", (255, 100, 30)),
            ("Dragon Claw", 45, "Fierce dragon slash!", (100, 200, 255)),
            ("Fire Spin", 35, "Trapping vortex of fire!", (255, 150, 50)),
            ("Blast Burn", 80, "Devastating fire explosion!", (255, 80, 20)),
        ],
        "color": (255, 120, 40),
    },
    "Blastoise": {
        "hp": 150, "atk": 50, "def": 55,
        "moves": [
            ("Water Gun", 35, "High-pressure water blast!", (50, 150, 255)),
            ("Ice Beam", 45, "Freezing cold ray!", (100, 220, 255)),
            ("Hydro Pump", 60, "Massive water cannon!", (30, 100, 255)),
            ("Skull Bash", 40, "Hard head charge!", (180, 180, 180)),
        ],
        "color": (60, 140, 220),
    },
}

ENEMY_POOL = [
    {
        "name": "Gengar", "hp": 100, "atk": 60, "def": 35,
        "moves": [
            ("Shadow Ball", 40, "A dark shadowy orb!", (150, 50, 200)),
            ("Sludge Bomb", 45, "Toxic sludge attack!", (120, 50, 180)),
            ("Night Shade", 50, "Eerie phantom strike!", (80, 30, 130)),
        ],
        "color": (130, 60, 180),
    },
    {
        "name": "Machamp", "hp": 130, "atk": 70, "def": 50,
        "moves": [
            ("Cross Chop", 55, "Devastating double chop!", (200, 150, 100)),
            ("Karate Chop", 35, "Swift martial strike!", (180, 140, 120)),
            ("Dynamic Punch", 60, "Explosive punch!", (220, 100, 50)),
        ],
        "color": (180, 140, 100),
    },
    {
        "name": "Alakazam", "hp": 90, "atk": 75, "def": 30,
        "moves": [
            ("Psychic", 55, "Mental energy blast!", (255, 100, 200)),
            ("Psybeam", 40, "Mysterious psychic ray!", (220, 80, 180)),
            ("Confusion", 30, "Disorienting mind wave!", (200, 100, 220)),
        ],
        "color": (200, 100, 180),
    },
]


class PokemonGame:
    def __init__(self, screen, clock):
        self.screen = screen
        self.clock = clock
        self.font_big = pygame.font.Font(None, 40)
        self.font = pygame.font.Font(None, 28)
        self.font_small = pygame.font.Font(None, 20)
        self.reset()

    def reset(self):
        self.state = "select"  # select, battle, win, lose
        self.player_pokemon = None
        self.player_name = ""
        self.player_max_hp = 100
        self.player_hp = 100
        self.enemy = None
        self.enemy_max_hp = 100
        self.enemy_hp = 100
        self.message = "Choose your Pokemon!"
        self.message_timer = 0
        self.battle_log = []
        self.anim_timer = 0
        self.player_hit = False
        self.enemy_hit = False
        self.move_buttons = []

    def select_pokemon(self, name):
        data = POKEMON[name]
        self.player_pokemon = data
        self.player_name = name
        self.player_max_hp = data["hp"]
        self.player_hp = data["hp"]
        # Pick random enemy
        self.enemy = random.choice(ENEMY_POOL).copy()
        self.enemy_max_hp = self.enemy["hp"]
        self.enemy_hp = self.enemy["hp"]
        self.state = "battle"
        self.message = f"Go, {name}! A wild {self.enemy['name']} appears!"

    def calc_damage(self, atk, def_val):
        base = atk * (random.randint(85, 100) / 100)
        reduction = def_val * 0.3
        return max(1, int(base - reduction))

    def player_attack(self, move_idx):
        if self.state != "battle":
            return
        move = self.player_pokemon["moves"][move_idx]
        damage = self.calc_damage(move[1], self.enemy["def"])
        self.enemy_hp -= damage
        self.enemy_hit = True
        self.anim_timer = 20
        self.message = f"{self.player_name} used {move[0]}! Dealt {damage} damage!"

        if self.enemy_hp <= 0:
            self.enemy_hp = 0
            self.state = "win"
            self.message = f"Enemy {self.enemy['name']} fainted! You win!"
            return

    def enemy_turn(self):
        """Called after player attack when enemy is still alive"""
        if self.state != "battle":
            return
        move = random.choice(self.enemy["moves"])
        damage = self.calc_damage(move[1], self.player_pokemon["def"])
        self.player_hp -= damage
        self.player_hit = True
        self.anim_timer = 20
        self.message = f"{self.enemy['name']} used {move[0]}! Dealt {damage} damage!"

        if self.player_hp <= 0:
            self.player_hp = 0
            self.state = "lose"
            self.message = f"{self.player_name} fainted... You lose!"

    def draw_pokemon_sprite(self, x, y, color, name, hp, max_hp, flipped=False):
        # Body
        body_rect = pygame.Rect(x - 25, y - 20, 50, 50)
        pygame.draw.ellipse(self.screen, color, body_rect)
        # Head
        pygame.draw.circle(self.screen, color, (x, y - 35), 22)
        # Eyes
        eye_off = -8 if flipped else 8
        pygame.draw.circle(self.screen, (255, 255, 255), (x + eye_off, y - 40), 7)
        pygame.draw.circle(self.screen, (0, 0, 0), (x + eye_off, y - 40), 4)
        # Arms/Legs
        arm_y = y - 5
        if flipped:
            pygame.draw.ellipse(self.screen, color, (x + 20, arm_y - 5, 30, 12))
        else:
            pygame.draw.ellipse(self.screen, color, (x - 50, arm_y - 5, 30, 12))
        # Name
        name_surf = self.font_small.render(name, True, TEXT_COLOR)
        self.screen.blit(name_surf, (x - name_surf.get_width() // 2, y + 35))

        # HP bar
        bar_w = 120
        bar_x = x - bar_w // 2
        bar_y = y + 55
        pygame.draw.rect(self.screen, (40, 40, 40), (bar_x, bar_y, bar_w, 12))
        hp_pct = hp / max_hp
        hp_color = HEALTH_GREEN if hp_pct > 0.5 else HEALTH_YELLOW if hp_pct > 0.2 else HEALTH_RED
        pygame.draw.rect(self.screen, hp_color, (bar_x, bar_y, int(bar_w * hp_pct), 12))
        hp_text = self.font_small.render(f"{hp}/{max_hp}", True, TEXT_COLOR)
        self.screen.blit(hp_text, (bar_x + bar_w // 2 - hp_text.get_width() // 2, bar_y - 1))

    def draw_battle_ui(self):
        # Background
        self.screen.fill(BG_COLOR)
        # Battlefield ground
        pygame.draw.ellipse(self.screen, (50, 100, 50), (100, 280, SCREEN_W - 200, 200))

        # Draw pokemon
        shake_x = 0
        if self.player_hit and self.anim_timer > 0:
            shake_x = math.sin(self.anim_timer * 0.8) * 8
        self.draw_pokemon_sprite(180, 230, self.player_pokemon["color"], self.player_name, self.player_hp, self.player_max_hp)
        enemy_x = SCREEN_W - 180 + shake_x
        self.draw_pokemon_sprite(enemy_x, 180, self.enemy["color"], self.enemy["name"], self.enemy_hp, self.enemy_max_hp, flipped=True)

        # Battle log
        log_y = 380
        pygame.draw.rect(self.screen, PANEL_COLOR, (40, log_y, SCREEN_W - 80, 80), border_radius=8)
        msg = self.font.render(self.message, True, TEXT_COLOR)
        self.screen.blit(msg, (60, log_y + 15))

        # Move buttons
        if self.state == "battle":
            btn_y = 475
            moves = self.player_pokemon["moves"]
            btn_w, btn_h = 190, 45
            total_w = len(moves) * btn_w + (len(moves) - 1) * 10
            start_x = SCREEN_W // 2 - total_w // 2

            mx, my = pygame.mouse.get_pos()

            for i, move in enumerate(moves):
                bx = start_x + i * (btn_w + 10)
                rect = pygame.Rect(bx, btn_y, btn_w, btn_h)
                hover = rect.collidepoint(mx, my)
                color = BUTTON_HOVER if hover else BUTTON_COLOR
                pygame.draw.rect(self.screen, color, rect, border_radius=8)
                pygame.draw.rect(self.screen, move[3], rect, 2, border_radius=8)

                name_surf = self.font.render(move[0], True, TEXT_COLOR)
                pwr_surf = self.font_small.render(f"Power: {move[1]}", True, (180, 180, 200))
                self.screen.blit(name_surf, (bx + 10, btn_y + 4))
                self.screen.blit(pwr_surf, (bx + 10, btn_y + 28))

    def draw_select_ui(self):
        self.screen.fill(BG_COLOR)
        title = self.font_big.render("Choose Your Pokemon", True, TEXT_COLOR)
        self.screen.blit(title, (SCREEN_W // 2 - title.get_width() // 2, 60))

        mx, my = pygame.mouse.get_pos()
        card_w, card_h = 220, 300
        gap = 30
        total_w = len(POKEMON) * card_w + (len(POKEMON) - 1) * gap
        start_x = SCREEN_W // 2 - total_w // 2

        for i, (name, data) in enumerate(POKEMON.items()):
            cx = start_x + i * (card_w + gap)
            rect = pygame.Rect(cx, 120, card_w, card_h)
            hover = rect.collidepoint(mx, my)
            color = BUTTON_HOVER if hover else PANEL_COLOR
            pygame.draw.rect(self.screen, color, rect, border_radius=16)
            pygame.draw.rect(self.screen, data["color"], rect, 3, border_radius=16)

            # Pokemon sprite
            self.draw_pokemon_sprite(cx + card_w // 2, 220, data["color"], name, data["hp"], data["hp"])

            # Stats
            atk_surf = self.font_small.render(f"ATK: {data['atk']}  DEF: {data['def']}", True, (200, 200, 220))
            self.screen.blit(atk_surf, (cx + card_w // 2 - atk_surf.get_width() // 2, 350))

            # Moves
            for j, move in enumerate(data["moves"][:3]):
                move_surf = self.font_small.render(f"{move[0]} ({move[1]})", True, (180, 180, 200))
                self.screen.blit(move_surf, (cx + 15, 375 + j * 22))

    def run(self):
        self.reset()
        pending_enemy_turn = False
        turn_delay = 0

        running = True
        while running:
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                if event.type == pygame.KEYDOWN:
                    if event.key == pygame.K_ESCAPE:
                        running = False
                    if event.key == pygame.K_r and self.state in ("win", "lose"):
                        self.reset()
                        pending_enemy_turn = False
                        turn_delay = 0

                if event.type == pygame.MOUSEBUTTONDOWN:
                    mx, my = pygame.mouse.get_pos()

                    if self.state == "select":
                        card_w, card_h = 220, 300
                        gap = 30
                        total_w = len(POKEMON) * card_w + (len(POKEMON) - 1) * gap
                        start_x = SCREEN_W // 2 - total_w // 2
                        for i, name in enumerate(POKEMON.keys()):
                            cx = start_x + i * (card_w + gap)
                            rect = pygame.Rect(cx, 120, card_w, card_h)
                            if rect.collidepoint(mx, my):
                                self.select_pokemon(name)

                    elif self.state == "battle" and not pending_enemy_turn:
                        moves = self.player_pokemon["moves"]
                        btn_w, btn_h = 190, 45
                        total_w = len(moves) * btn_w + (len(moves) - 1) * 10
                        start_x = SCREEN_W // 2 - total_w // 2
                        for i in range(len(moves)):
                            bx = start_x + i * (btn_w + 10)
                            rect = pygame.Rect(bx, 475, btn_w, btn_h)
                            if rect.collidepoint(mx, my):
                                self.player_attack(i)
                                if self.state == "battle":
                                    pending_enemy_turn = True
                                    turn_delay = 60  # frames delay before enemy moves

            # Handle turn delay
            if pending_enemy_turn:
                turn_delay -= 1
                if turn_delay <= 0:
                    self.enemy_turn()
                    pending_enemy_turn = False

            # Animation timer
            if self.anim_timer > 0:
                self.anim_timer -= 1

            if self.state == "select":
                self.draw_select_ui()
            else:
                self.draw_battle_ui()

            # Win/Lose overlay
            if self.state == "win":
                overlay = pygame.Surface((SCREEN_W, SCREEN_H), pygame.SRCALPHA)
                overlay.fill((0, 150, 50, 100))
                self.screen.blit(overlay, (0, 0))
                win_surf = self.font_big.render("YOU WIN!", True, (255, 255, 0))
                hint = self.font.render("Press R to play again", True, TEXT_COLOR)
                self.screen.blit(win_surf, (SCREEN_W // 2 - win_surf.get_width() // 2, 150))
                self.screen.blit(hint, (SCREEN_W // 2 - hint.get_width() // 2, 210))
            elif self.state == "lose":
                overlay = pygame.Surface((SCREEN_W, SCREEN_H), pygame.SRCALPHA)
                overlay.fill((150, 0, 0, 100))
                self.screen.blit(overlay, (0, 0))
                lose_surf = self.font_big.render("YOU LOSE!", True, (255, 100, 100))
                hint = self.font.render("Press R to try again", True, TEXT_COLOR)
                self.screen.blit(lose_surf, (SCREEN_W // 2 - lose_surf.get_width() // 2, 150))
                self.screen.blit(hint, (SCREEN_W // 2 - hint.get_width() // 2, 210))

            # ESC hint
            esc_surf = self.font_small.render("ESC to return to menu", True, (120, 120, 140))
            self.screen.blit(esc_surf, (SCREEN_W - esc_surf.get_width() - 15, SCREEN_H - 25))

            pygame.display.flip()
            self.clock.tick(60)

        self.reset()
