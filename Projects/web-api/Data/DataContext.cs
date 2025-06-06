using Microsoft.EntityFrameworkCore;
using rpg_game.Models;

namespace rpg_game.Data;
/*
 we can use this instance to query the database
 and save all the changes to our RPG characters
 */
public class DataContext : DbContext
{
 public DataContext(DbContextOptions<DataContext> options) : base(options) { }
 public DbSet<Character> Characters { get; set; } 
 public DbSet<User> Users { get; set; }
 public DbSet<Weapon> Weapons { get; set; }
 public DbSet<Skill> Skills { get; set; }
 public DbSet<CharacterSkill> CharacterSkills { get; set; }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
  modelBuilder.Entity<CharacterSkill>()
   .HasKey(cs => new { cs.CharacterId, cs.SkillId });
 }
}