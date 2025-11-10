using System.ComponentModel.DataAnnotations;

public enum UnitType
{
    [Display(Name = "Grama (g)")]
    Grama = 1,

    [Display(Name = "Quilograma (kg)")]
    Quilograma = 2,

    [Display(Name = "Mililitro (ml)")]
    Mililitro = 3,

    [Display(Name = "Litro (L)")]
    Litro = 4,

    [Display(Name = "Unidade (un)")]
    Unidade = 5,

    [Display(Name = "Colher de Sopa")]
    ColherDeSopa = 6,

    [Display(Name = "Colher de Cha")]
    ColherDeCha = 7,

    [Display(Name = "Xicara")]
    Xicara = 8,

    [Display(Name = "Hora")]
    Hora = 9
}
